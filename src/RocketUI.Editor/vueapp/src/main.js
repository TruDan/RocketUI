import Vue from 'vue'
import App from './App.vue'
import plugins from './plugins';

import '@/styles/main.scss';
import 'splitpanes/dist/splitpanes.css';
import store from './store'

Vue.config.productionTip = false

Vue.filter('capitalize', function (value) {
  if (!value) return ''
  value = value.toString()
  return value.charAt(0).toUpperCase() + value.slice(1)
})

new Vue({
  ...plugins,
  store,
  render: h => h(App)
}).$mount('#app')
