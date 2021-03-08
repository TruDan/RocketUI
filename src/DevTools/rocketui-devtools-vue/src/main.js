import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify';
import rocketws from './plugins/rocketdebugger';

import './sass/main.scss'
import store from './store'

Vue.config.productionTip = false


new Vue({
  vuetify,
  rocketws,
  store,
  render: h => h(App)
}).$mount('#app')
