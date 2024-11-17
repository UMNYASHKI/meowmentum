ARG NGINX_VERSION=stable

FROM nginx:${NGINX_VERSION}

COPY --from=gen /nginx.conf /etc/nginx/conf.d/default.conf

CMD ["nginx", "-g", "daemon off;"]
