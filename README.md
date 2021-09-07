# Cloud Event to Amplitude Event Forwarder

This microservce / serverless function helps companies with and existing eventing systems conformant to the [CloudEvents](https://cloudevents.io/) specification to [Amplitude Analytics](https://amplitude.com/).  This service can stood up as an event sink for managed event services such as Azure EventGrid, GCP Pub/Sub, AWS EventBridge, or Knative Eventing.

**Please note**: This is works for Presalytics to send events to Ampliude.  We'd welocome PRs to upgrade the service and feature suggestions.

# Quick Start

````bash
docker run -p 8080:80 -e AMPLITUDE_API_KEY=${YOUR_API_KEY} presalytics/amplitude
````

This bash command quickly stands up local server for testing.

# Proudction 

Please refer to the your Cloud provider's docs for how set up a container instance or serverless functiont that listens for events from your event hub service.  For Knative an example Knative Serving service and Eventing trigger can be found in the [manifests directory](manifests/)

# Contributing

This is an early stage project, so please help, raise an issue, and provide advice.  Just be a nice and inclusive person (or bot) when you do.  Thanks!