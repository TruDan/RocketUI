import Vue from 'vue'
import Vuex from 'vuex'
import elementTree from './modules/elementTree';

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    elementTree
  }
})
