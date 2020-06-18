<template>
    <v-row>
        <v-col cols="12">
            <v-data-table
                :headers="headers"
                :items="microservices"
                :items-per-page="5"
                :disable-pagination="disablePagination">
                <template v-slot:top>
                    <v-toolbar flat>
                        <v-toolbar-title>
                            <v-icon color="red">mdi-ethernet-cable</v-icon>
                            Endpoints
                        </v-toolbar-title>
                    </v-toolbar>
                </template>
                <template v-slot:item.actions="{ item }">
                    <v-tooltip top color="cyan">
                        <template v-slot:activator="{ on }">
                        <v-btn icon dark color="cyan" v-on="on" :to="{ name: 'endpoint-details', params: { id: item.id } }">
                            <v-icon small>mdi-magnify</v-icon>
                        </v-btn>
                        </template>
                        <span>Details</span>
                    </v-tooltip>
                </template>
            </v-data-table>
        </v-col>
    </v-row>    
</template>

<script>
    export default {
        data () {
            return {
                page: 1,
                pageCount: 6,
                disablePagination: true,
                alignment: 'start',
                headers: [
                    { text: 'URL', value: 'url' },
                    { text: 'Actions', value: 'actions', sortable: false, align: 'end' },
                ],
                microservices: []
            }
        },
        created () {
            this.$http.get('https://localhost:5001/api/v1/endpoints?page=1&itemsPerPage=5')
                .then(response => {
                    this.microservices = response.body;                    
                }, error => {
                    debugger
                    console.log(error)
                })
        },
        computed: {

        },
        methods: {
            closeDialog () {
                this.dialog = false
                
                this.dialogData = {
                    index: -1,
                    url: null,
                    clientKey: null,
                    clientSecret: null
                }
            }
        }
    }
</script>