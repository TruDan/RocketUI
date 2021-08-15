<template>
  <v-app class="app">
    <error-list/>
    <v-system-bar app dark>
      <RocketWSStatus/>
    </v-system-bar>

    <v-main class="main">
      <DevToolsMain/>
    </v-main>

  </v-app>
</template>

<script>
import DevToolsMain from './components/DevToolsMain';
import RocketWSStatus from "@/components/RocketWSStatus";
import ErrorList from "@/components/ErrorList";

export default {
  name: 'App',

  components: {
    ErrorList,
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
  max-height: 100vh;
  height: 100vh;
}

.main > div {
  display: block;
}
</style>
