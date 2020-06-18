<template>
    <v-row>
        <v-col cols="12">
            <v-card>
                <v-data-table
                    :headers="headers"
                    :items="microservices"
                    :items-per-page="5">
                    <template v-slot:top>
                        <v-toolbar flat>
                            <v-toolbar-title>
                                <v-icon color="cyan">mdi-hexagon-multiple</v-icon>
                                Microservices
                            </v-toolbar-title>
                            <v-spacer></v-spacer>
                            <v-dialog v-model="dialog" max-width="500px">
                                <template v-slot:activator="{ on }">
                                    <v-btn color="cyan" v-on="on">
                                        <v-icon>mdi-plus</v-icon>
                                        Add microservice
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-title>
                                        <v-icon class="mr-2" color="cyan">mdi-hexagon</v-icon>
                                        Add microservice
                                    </v-card-title>
                                    <v-card-text>
                                        <v-container>
                                            <v-row>
                                                <v-col cols="12">
                                                    <v-text-field v-model="dialogData.url" label="URL"></v-text-field>
                                                </v-col>
                                            </v-row>
                                            <v-row>
                                                <v-col cols="12">
                                                    <v-text-field v-model="dialogData.clientKey" label="Client Key"></v-text-field>
                                                </v-col>
                                            </v-row>
                                            <v-row>
                                                <v-col cols="12">
                                                    <v-text-field v-model="dialogData.clientSecret" label="Client Secret"></v-text-field>
                                                </v-col>
                                            </v-row>
                                        </v-container>
                                    </v-card-text>
                                    <v-card-actions>
                                        <v-spacer></v-spacer>
                                        <v-btn @click="closeDialog" text color="grey lighter-2">Cancel</v-btn>
                                        <v-btn @click="saveMicroservice" text color="cyan">
                                            <v-icon>mdi-content-save-outline</v-icon>
                                            Save
                                        </v-btn>
                                    </v-card-actions>
                                </v-card>
                            </v-dialog>
                        </v-toolbar>
                    </template>
                    <template v-slot:item.actions="{ item }">
                        <v-tooltip top color="cyan">
                            <template v-slot:activator="{ on }">
                            <v-btn icon dark color="cyan" v-on="on">
                                <v-icon small to="/microservice/1">mdi-magnify</v-icon>
                            </v-btn>
                            </template>
                            <span>Details</span>
                        </v-tooltip>
                        <v-tooltip top color="warning">
                            <template v-slot:activator="{ on }">
                            <v-btn icon dark color="warning" v-on="on">
                                <v-icon small @click="editMicroservice(item)">mdi-pencil</v-icon>
                            </v-btn>
                            </template>
                            <span>Edit</span>
                        </v-tooltip>
                        <v-tooltip top color="error">
                            <template v-slot:activator="{ on }">
                            <v-btn icon dark color="error" v-on="on">
                                <v-icon small @click="removeMicroservice(item)">mdi-delete</v-icon>
                            </v-btn>
                            </template>
                            <span>Delete</span>
                        </v-tooltip>
                    </template>
                </v-data-table>
            </v-card>
        </v-col>
    </v-row>    
</template>

<script>
    export default {
        data () {
            return {
                alignment: 'start',
                headers: [
                    { text: 'Name', value: 'name' },
                    { text: 'Description', value: 'description' },
                    { text: 'URL', value: 'url' },
                    { text: 'Actions', value: 'actions', sortable: false },
                ],
                microservices: [
                    {
                        name: 'Microservice_1',
                        description: 'Microservice_1 description',
                        url: 'https://microservice_1.com',
                    }
                ],
                dialog: false,
                dialogData: {
                    index: -1,
                    url: null,
                    clientKey: null,
                    clientSecret: null
                }
            }
        },
        created () {
            this.$http.get('https://localhost:44393/api/v1/microservices')
                .then(response => {
                    debugger
                    console.log(response)
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
            },
            saveMicroservice () {
                // TODO: Authenticate if client key and secret are provided
                // TODO: Go to URL to collect microservice discovery data
                // TODO: Write to database
                // TODO: perform below action to update data table
                if (this.dialogData.index > -1) {
                    var microservice = this.microservices[this.dialogData.index]

                    microservice.url = this.dialogData.url
                } else {
                    this.microservices.push({
                        name: 'Microservice_2',
                        description: 'Microservice_2 description',
                        url: this.dialogData.url
                    })
                }

                this.dialogData = {
                    index: -1,
                    url: null,
                    clientKey: null,
                    clientSecret: null
                }

                this.dialog = false
            },
            editMicroservice (microservice) {
                this.dialogData.index = this.microservices.indexOf(microservice)

                this.dialogData.url = microservice.url

                this.dialog = true
            },

            removeMicroservice (microservice) {
                const index = this.microservices.indexOf(microservice)

                this.microservices.splice(index, 1)
            }
        }
    }
</script>