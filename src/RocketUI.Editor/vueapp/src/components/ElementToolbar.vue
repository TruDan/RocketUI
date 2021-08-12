<template>
  <v-toolbar>
    <v-spacer/>
    <v-btn icon
           @click="refreshTree()">
      <v-icon>mdi-refresh</v-icon>
    </v-btn>
  </v-toolbar>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "ElementToolbar",

  methods: {
    ...mapActions('elementTree', ['getRootElement']),

    refreshTree() {
      this.getRootElement();
    }
  }
}
</script>

<style scoped>

</style>
