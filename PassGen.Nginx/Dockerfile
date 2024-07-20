FROM nginx:latest
RUN mv /etc/nginx/conf.d/default.conf /etc/nginx/conf.d/default.conf.bak
COPY ./nginx.conf /etc/nginx/conf.d/passgen.conf
COPY ./wwwroot /data/wwwroot