g<template>
  <v-app class="app">
    <v-system-bar app dark>
      <RocketWSStatus />
    </v-system-bar>

    <v-main class="main">
      <DevToolsMain />
    </v-main>

  </v-app>
</template>

<script>
import DevToolsMain from './components/DevToolsMain';
import RocketWSStatus from "@/components/RocketWSStatus";

export default {
  name: 'App',

  components: {
    RocketWSStatus,
    DevToolsMain,
  },

  data: () => ({
    //
  }),
};
</script>

<style lang="scss">
.app {
  min-height: 100vh;
}

.main > div {
  display: flex;
}
</style>