# Stage 1: Build Angular app
FROM node:20 AS build
WORKDIR /app

# Copy package files and install dependencies
COPY DigitalBankLite.Client/package*.json ./
RUN npm install

# Copy source and build
COPY DigitalBankLite.Client/ .
RUN npm run build -- --configuration production

# Stage 2: Serve with Nginx
FROM nginx:alpine
COPY --from=build /app/dist/digital-bank-lite.client/browser /usr/share/nginx/html
COPY devops/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
