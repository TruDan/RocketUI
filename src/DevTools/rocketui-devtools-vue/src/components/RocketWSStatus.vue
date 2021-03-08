<template>
  <v-icon v-bind:class="status">mdi-flash-circle</v-icon>
</template>

<script>
export default {
  name: "RocketWSStatus",

  data: () => {
    const status = () => {
      switch (window.$rocketws.getStatus()) {
        case WebSocket.CONNECTING:
          return 'ws-green ws-pending';
        case WebSocket.OPEN:
          return 'ws-green';
        case WebSocket.CLOSING:
          return 'ws-red ws-pending';
        case WebSocket.CLOSED:
          return 'ws-red';
        default:
          return 'ws-red ws-pending';
      }
    };

    return {
      status: status().split(' ')
    }
  }
}
</script>

<style scoped lang="scss">

@import 'src/sass/main';

@debug $colors;

.ws-green {
  color: var(--v-success-base) !important;
}

.ws-red {
  color: var(--v-error-base) !important;
}

@keyframes ws-pending-animation {
  from {
    opacity: 1;
  }
  to {
    opacity: 0.25;
  }
}

.ws-pending {
  animation: ws-pending-animation alternate-reverse ease-in-out 1s infinite;
}

</style>