FROM flyway/flyway:latest 
COPY ./flyway.conf /flyway/conf

ENTRYPOINT [ "flyway", "migrate" ]