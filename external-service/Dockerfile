FROM node:22-alpine
WORKDIR /app
COPY server.mjs ./
USER node
ENV PORT=4000
EXPOSE 4000
CMD ["node", "server.mjs"]
