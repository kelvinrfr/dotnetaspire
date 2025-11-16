FROM gcr.io/google.com/cloudsdktool/google-cloud-cli:emulators

RUN gcloud config set project local-emulator
CMD ["gcloud", "beta", "emulators", "pubsub", "start", "--project=local-emulator", "--host-port=0.0.0.0:8085"]
