<template>
    <v-card>
        <v-card-title>
            <v-icon color="red">mdi-ethernet-cable</v-icon> Endpoint
        </v-card-title>
        <v-card-text>
            <v-tabs color="red" v-model="tab">
                <v-tab>Details</v-tab>
                <v-tab-item>
                    <endpoint-tab-details></endpoint-tab-details>
                </v-tab-item>
                <v-tab>Event</v-tab>
                <v-tab-item>
                    <endpoint-tab-events></endpoint-tab-events>
                </v-tab-item>
            </v-tabs>
        </v-card-text>
    </v-card>
</template>

<script>
    import EndpointTabDetails from "@/components/endpoints/EndpointTabDetails"
    import EndpointTabEvents from "@/components/endpoints/EndpointTabEvents"

    export default {
        components: {
            EndpointTabDetails,
            EndpointTabEvents
        },
        data () {
            return {
                tab: null,
                items: [
                    { tab: 'endpoint details', content: '123' },
                    { tab: 'events', content: '456' }
                ]
            }
        }
    }
</script>