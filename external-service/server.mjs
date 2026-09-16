import { createServer } from "node:http";

const port = Number(process.env.PORT ?? 4000);
const failureRate = Number(process.env.FAILURE_RATE ?? 0);
const loanApplications = new Map();
const itemRoute = /^\/loan-applications\/([^/]+)$/;

const maskSsn = (ssn = "") => `***-**-${String(ssn).slice(-4)}`;

const send = (response, status, body) => {
  response.writeHead(status, { "Content-Type": "application/json" });
  response.end(JSON.stringify(body));
};

const readJson = async (request) => {
  const chunks = [];
  for await (const chunk of request) chunks.push(chunk);
  return JSON.parse(Buffer.concat(chunks).toString("utf8"));
};

const log = (request, status, outcome, payload) => {
  const summary = payload
    ? { ...payload, customer: { ...payload.customer, ssn: maskSsn(payload.customer?.ssn) } }
    : undefined;
  console.log(`${new Date().toISOString()} ${request.method} ${request.url} -> ${status} ${outcome}`);
  if (summary) console.log(JSON.stringify(summary, null, 2));
};

const createLoanApplication = (request, response, payload) => {
  const exists = loanApplications.has(payload.applicationId);
  loanApplications.set(payload.applicationId, { ...payload, receivedAt: new Date().toISOString() });
  const outcome = exists ? "replayed" : "created";
  log(request, 200, outcome, payload);
  send(response, 200, { applicationId: payload.applicationId, outcome });
};

const updateLoanApplication = (request, response, applicationId, payload) => {
  if (payload.applicationId !== applicationId) {
    log(request, 400, "id mismatch");
    return send(response, 400, { error: "applicationId in the body must match the URL" });
  }
  if (!loanApplications.has(applicationId)) {
    log(request, 404, "not found");
    return send(response, 404, { error: "loan application not found" });
  }
  loanApplications.set(applicationId, { ...payload, receivedAt: new Date().toISOString() });
  log(request, 200, "updated", payload);
  send(response, 200, { applicationId, outcome: "updated" });
};

const server = createServer(async (request, response) => {
  const { pathname } = new URL(request.url, "http://localhost");

  if (request.method === "GET" && pathname === "/loan-applications") {
    return send(response, 200, [...loanApplications.values()]);
  }

  const isCreate = request.method === "POST" && pathname === "/loan-applications";
  const updateMatch = request.method === "PUT" ? pathname.match(itemRoute) : null;

  if (!isCreate && !updateMatch) {
    return send(response, 404, { error: "route not found" });
  }

  if (Math.random() < failureRate) {
    log(request, 503, "simulated failure");
    return send(response, 503, { error: "simulated failure" });
  }

  let payload;
  try {
    payload = await readJson(request);
  } catch {
    log(request, 400, "invalid json");
    return send(response, 400, { error: "body must be valid JSON" });
  }

  if (!payload?.applicationId || !payload?.customer) {
    log(request, 400, "missing applicationId or customer");
    return send(response, 400, { error: "applicationId and customer are required" });
  }

  return isCreate
    ? createLoanApplication(request, response, payload)
    : updateLoanApplication(request, response, decodeURIComponent(updateMatch[1]), payload);
});

server.listen(port, () => {
  console.log(`External loan service mock listening on http://localhost:${port} (failure rate ${failureRate})`);
});
