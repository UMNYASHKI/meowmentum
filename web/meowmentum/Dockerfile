FROM node:20-alpine as builder

WORKDIR /app
COPY package.json yarn.lock ./

RUN yarn install --frozen-lockfile

COPY . .

ARG NEXT_PUBLIC_API_ENDPOINT
ENV NEXT_PUBLIC_API_ENDPOINT $NEXT_PUBLIC_API_ENDPOINT
ENV NODE_ENV production

RUN yarn run build

FROM node:20-alpine as runner


WORKDIR /app

COPY --from=builder /app/package.json ./
COPY --from=builder /app/yarn.lock ./

COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

#COPY --from=builder /app/public ./public

EXPOSE 3000

CMD ["node", "server.js"]