# Loan Application Flow

> **Video walkthrough:** _link coming soon_

A small loan application flow. A Next.js form sends the application to a .NET API. A rule engine approves or denies it. Approved applications are saved in PostgreSQL in one transaction together with an event, and a background processor then sends that event to an external service over HTTP.

- **Design decisions, the rule engine, the event and the transaction:** [ARCHITECTURE.md](ARCHITECTURE.md)
- **Stack:** .NET 10 · EF Core 10 · PostgreSQL 17 · Next.js 16 · Tailwind CSS 4 · zod · Node (external service mock)

## Run everything with Docker

Requirements: Docker with Compose.

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| Web app | http://localhost:3000 |
| API | http://localhost:8080 (OpenAPI document at `/openapi/v1.json`) |
| External service mock | http://localhost:4000/loan-applications |
| PostgreSQL | `localhost:5432`, database, user and password `loanflow` |

The API applies its database migrations on startup, so there's no setup step.

## Run the apps locally

Requirements: .NET 10 SDK, Node.js 20.9+ and Docker (for PostgreSQL and the integration tests).

Start PostgreSQL and the external service mock:

```bash
docker compose up -d postgres external-service
```

Start the API on http://localhost:8080:

```bash
dotnet run --project backend/src/LoanFlow.Api
```

Start the web app on http://localhost:3000:

```bash
cd frontend && npm install && npm run dev
```

The web app calls the API at `http://localhost:8080` unless `API_URL` is set. You can also run the mock without Docker with `node external-service/server.mjs`. `backend/src/LoanFlow.Api/LoanFlow.Api.http` has ready-made requests for every scenario.

## Tests

Backend unit and integration tests. The integration tests start PostgreSQL with Testcontainers, so Docker must be running:

```bash
dotnet test backend/LoanFlow.slnx
```

Frontend tests (schema, input formatting and denial copy):

```bash
cd frontend && npm test
```

| Suite | What it covers |
|---|---|
| `LoanFlow.UnitTests` | Rule engine (NY, blacklist in any SSN format, all reasons collected, a new rule plugged in), value objects, the `Customer` aggregate, the submit use case for new, returning, denied and invalid applications, create vs update sync, rule discovery. |
| `LoanFlow.IntegrationTests` | `POST /api/loan-applications` for approved, denied and invalid input; returning customer updates the same rows; **transaction rollback** of the new and returning paths when the outbox insert fails; outbox delivery (POST for new, PUT for returning, backoff on failure, timeout); unique SSN conflict. |
| `frontend` | Validation schema, SSN and amount formatting, denial reason copy. |

CI (GitHub Actions) runs all of them on every pull request.

## Test data

| Scenario | What to enter | What you should see |
|---|---|---|
| **Approved, new customer** | Any state except New York, an SSN that is not blacklisted, e.g. `521-45-7788` | "Your application is approved". The mock logs a `POST`. |
| **Returning customer** | Submit again with the **same SSN** and a different amount or address | "Your application is approved and updated". Still one customer and one application in the database. The mock logs a `PUT`. |
| **Denied by state** | State **New York** | "We don't lend in your state yet". Nothing saved. |
| **Denied by blacklist** | SSN `123-45-6789`, `111-11-1111` or `999-99-9999` | "We couldn't verify your identity". Nothing saved. |

SSNs can be typed with or without dashes. The blacklist is in `backend/src/LoanFlow.Api/appsettings.json` under `SsnBlacklist:Ssns`.

## See what happened

What the external service received:

```bash
curl -s localhost:4000/loan-applications
```

```bash
docker compose logs -f external-service
```

What is in the database:

```bash
docker compose exec postgres psql -U loanflow -d loanflow -c "select c.ssn, c.first_name, c.last_name, c.address_state, a.id as application_id, a.requested_amount from customers c join loan_applications a on a.customer_id = c.id"
```

```bash
docker compose exec postgres psql -U loanflow -d loanflow -c "select occurred_at, processed_at, attempts, last_error from outbox_messages order by occurred_at"
```

To watch the retries, stop the external service, submit an application, and start the service again. The pending message is delivered automatically:

```bash
docker compose stop external-service
```

```bash
docker compose start external-service
```

The mock can also fail randomly. `FAILURE_RATE` is a value between 0 and 1:

```bash
FAILURE_RATE=0.5 docker compose up -d external-service
```

## API

```bash
curl -s -X POST localhost:8080/api/loan-applications -H 'Content-Type: application/json' -d '{
  "firstName": "Ada", "lastName": "Lovelace", "companyName": "Analytical Engines LLC",
  "address": { "street": "100 Congress Ave", "city": "Austin", "state": "TX", "zipCode": "78701" },
  "ssn": "521-45-7788", "requestedAmount": 25000
}'
```

| Outcome | Status | Body |
|---|---|---|
| Approved | `200` | `{ "decision": "approved", "applicationId": "…", "isReturningCustomer": false }` |
| Denied | `200` | `{ "decision": "denied", "reasons": [{ "code": "state-not-served", "message": "…" }] }` |
| Invalid input | `400` | problem details with the validation message |
| Same new SSN submitted twice at the same moment | `409` | problem details |

## Known limitations

These are deliberate, and each one is explained in the trade-offs section of [ARCHITECTURE.md](ARCHITECTURE.md):

- The SSN is stored in plain text. In production it would be encrypted at rest and looked up through a keyed hash.
- The outbox processor assumes a single API instance.
- There is no authentication, and the response reveals whether an SSN was already known.
- Processed outbox messages are never cleaned up, and there is no admin view for messages that exhausted their retries.
