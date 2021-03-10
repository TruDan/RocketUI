<template>
  <v-treeview
      :items="rootElements"
      dense
      open-all
      activatable
      expand-icon="mdi-menu-down"

      :active.sync="active">
    <template v-slot:label="{ item }">
      <pre>{{ item.display || item.name || '' }}</pre>
    </template>
  </v-treeview>
</template>

<script>
import {mapState, mapActions} from 'vuex';

export default {
  name: "ScreenTreeView",

  // data: () => {
  //   var data = Vue.$rocketws.send('{"Id": 1, "Command": "GetRoot", "Arguments": []}');
  //   Vue.$rocketws.addEventListener('message', ev => {
  //     var deserialized = JSON.parse(ev.data);
  //     if(deserialized.Id === 1) {
  //
  //     }
  //   })
  // }
  computed: mapState({
    rootElements: state => state.elementTree.rootElements,
  }),
  watch: {
    active: function(val) {
      if(!val.length) return;
      const id = val[0];
      this.$store.dispatch('elementTree/selectElement', id);
    }
  },
  methods: mapActions('elementTree', [
    'selectElement'
  ]),
  created() {
    this.$store.dispatch('elementTree/getRootElement');
  },
  data: () => ({
    componentTree: [],
    active: []
  }),
}
</script>

<style lang="scss">
.v-treeview--dense .v-treeview-node__root {
  min-height: 1rem;
}
</style>