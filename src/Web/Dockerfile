FROM node:24-alpine AS build_angular
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:1.31-alpine AS config_nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build_angular /app/dist/recipes-web/browser /usr/share/nginx/html

EXPOSE 80
