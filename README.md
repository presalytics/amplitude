# Cloud Event to Amplitude Event Forwarder

This microservce / serverless function helps companies with existing eventing infrastucture conformant to the [CloudEvents](https://cloudevents.io/) specification to forward their events to [Amplitude Analytics](https://amplitude.com/).  For example, This service can be stood up as an event sink for managed event services (e.g, Azure EventGrid, GCP Pub/Sub, AWS EventBridge, or Knative Eventing) to quickly piggyback Amplitude over the top of an existing tech stack.

**Please note**: This is works for Presalytics to send events to Amplitude.  We build this to quickly integrate Amplitude with our eventing system built with Knative.  We'd welocome PRs to upgrade the service so that it can be generalized to other organizations with different event taxonomies and event hub services.

# Quick Start

````bash
docker run -p 8080:80 -e AMPLITUDE_API_KEY=${YOUR_API_KEY} presalytics/amplitude
````

This bash command quickly stands up a local server for testing purposes.

# Production

Please refer to the your Cloud provider's docs for how set up a container instance or serverless function that listens for events from your event hub service.  For Knative, an example Knative Serving service and Eventing trigger can be found in the [manifests directory](manifests/)

# Contributing

This is an early stage project, so please help, raise an issue, and provide advice.  Just be a nice and inclusive person (or bot) when you do.  Thanks!
