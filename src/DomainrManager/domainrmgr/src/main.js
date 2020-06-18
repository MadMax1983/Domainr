import Vue from 'vue'
import router from "./plugins/router";
import vueResource from './plugins/vue-resource';
import vuetify from './plugins/vuetify';
import App from './App.vue'

Vue.config.productionTip = false

new Vue({
  router,
  vueResource,
  vuetify,
  render: h => h(App)
}).$mount('#app')