FROM ghcr.io/neoscript/pubsub-emulator-ui:latest AS static-files

FROM nginx:alpine

# Remove default nginx files
RUN rm -rf /usr/share/nginx/html/*

# Copy all static files from the source image's nginx html directory
COPY --from=static-files /usr/share/nginx/html/ /usr/share/nginx/html/

# Set proper ownership and permissions
RUN chown -R nginx:nginx /usr/share/nginx/html && \
    chmod -R 755 /usr/share/nginx/html && \
    find /usr/share/nginx/html -type f -exec chmod 644 {} \;

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
