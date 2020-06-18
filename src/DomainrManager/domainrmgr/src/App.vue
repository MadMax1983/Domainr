<template>
  <v-app id="inspire">
    <v-navigation-drawer
      v-model="drawer"
      app
      clipped
    >
      <v-list dense>
        <v-list-item link to="/dashboard">
          <v-list-item-action>
            <v-icon color="purple lighten-2">mdi-gauge</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Dashboard</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link to="/microservices">
          <v-list-item-action>
            <v-icon color="cyan">mdi-hexagon-multiple</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Microservices</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link to="/endpoints">
          <v-list-item-action>
            <v-icon color="red">mdi-ethernet-cable</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Endpoints</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link to="/accounts">
          <v-list-item-action>
            <v-icon color="light-green accent-2">mdi-account-multiple</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Accounts</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link to="/settings">
          <v-list-item-action>
            <v-icon color="amber lighten-1">mdi-cogs</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Settings</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar
      app
      clipped-left
    >
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title>Domainr Manager</v-toolbar-title>

      <v-spacer></v-spacer>

      <v-tooltip bottom>
        <template v-slot:activator="{ on }">
        <v-btn icon dark v-on="on">
          <v-icon>mdi-account</v-icon>
        </v-btn>
        </template>
        <span>Account</span>
      </v-tooltip>

      <v-tooltip bottom>
        <template v-slot:activator="{ on }">
          <v-btn icon dark v-on="on">
            <v-icon>mdi-login-variant</v-icon>
          </v-btn>
        </template>
        <span>Sign In</span>
      </v-tooltip>

      <v-tooltip bottom>
        <template v-slot:activator="{ on }">
          <v-btn icon dark v-on="on">
            <v-icon>mdi-account-plus</v-icon>
          </v-btn>
        </template>
        <span>Sign Up</span>
      </v-tooltip>

      <v-tooltip bottom>
        <template v-slot:activator="{ on }">
          <v-btn icon dark v-on="on">
            <v-icon>mdi-logout-variant</v-icon>
          </v-btn>
        </template>
        <span>Sign Out</span>
      </v-tooltip>      
    </v-app-bar>

    <v-content>
      <v-container
        fluid
      >
        <router-view></router-view>
      </v-container>
    </v-content>

    <v-footer app>
      <span>&copy; 2020</span>
    </v-footer>
  </v-app>
</template>

<script>
  export default {
    components: {
    },
    props: {
      source: String,
    },
    data: () => ({
      drawer: null,
    }),
    created () {
      this.$vuetify.theme.dark = true
    },
  }
</script>
