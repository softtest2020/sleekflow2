FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publish
COPY . /app
WORKDIR /app/Ticket.WebApi
RUN dotnet publish -c Release -o /app/webapp-publish

FROM fromYourOwnImage AS final
RUN groupadd -g 999 appuser && \
    useradd -r -u 999 -g appuser appuser
ENV ASPNETCORE_URLS=http://*:5000
WORKDIR /app
COPY --from=publish /app/webapp-publish .
COPY BuildConfig/Templates/Kubernetes/Setup.sh Setup.sh
RUN chown -R appuser:appuser /app
EXPOSE 5000
USER 999
RUN chmod +x Setup.sh
RUN mkdir Logs
ENTRYPOINT ["sh", "Setup.sh"]
