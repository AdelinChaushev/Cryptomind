# Stage 1: Build the React app
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files first (better layer caching)
COPY package.json package-lock.json ./
RUN npm ci

# Copy source and build
COPY . .
RUN npm run build -- --mode production

# Stage 2: Serve with Nginx
# The build output (dist/) is copied into Nginx which serves it as static files
FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

# Custom nginx config to handle React Router (client-side routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
