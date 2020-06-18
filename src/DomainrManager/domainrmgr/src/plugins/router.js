import Vue from "vue";
import VueRouter from "vue-router";

import Dashboard from "@/components/Dashboard"
import Microservices from "@/components/Microservices"
import EndpointList from "@/components/endpoints/EndpointList"
import EndpointDetails from "@/components/endpoints/EndpointDetails"
import Accounts from "@/components/Accounts"
import Settings from "@/components/Settings"

Vue.use(VueRouter);

export default new VueRouter({
  routes: [
    {
      path: "/",
      name: "home",
      component: Dashboard
    },
    {
      path: "/dashboard",
      name: "dashboard",
      component: Dashboard
    },
    {
      path: "/microservices",
      name: "microservices",
      component: Microservices
    },
    {
      path: "/endpoints",
      name: "endpoints",
      component: EndpointList
    },
    {
      path: "/endpoints/:id",
      name: "endpoint-details",
      component: EndpointDetails
    },
    {
      path: "/accounts",
      name: "Accounts",
      component:Accounts
    },
    {
      path: "/settings",
      name: "Settings",
      component: Settings
    }
  ]
});
