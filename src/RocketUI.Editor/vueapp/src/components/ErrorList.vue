<template>
  <div class="errorlist">
    <v-alert v-for="(error, i) in errors"
             :key="i"
             type="error"
             dense
             dismissible
    >
      {{ error }}
    </v-alert>
  </div>
</template>

<script>
import {mapState} from "vuex";

export default {
  name: "ErrorList",

  computed: {
    ...mapState('elementTree', {
      errors: state => state.errors
    })
  }
}
</script>

<style scoped>
.errorlist {
  position: fixed;
  top: 3rem;
  z-index: 9999;
}
</style>
