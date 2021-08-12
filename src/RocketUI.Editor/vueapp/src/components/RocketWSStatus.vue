<template>
  <v-container fluid>
  <v-icon v-bind:class="status">mdi-flash-circle</v-icon>
  <span>{{ statusText }}</span>
  </v-container>
</template>

<script>
import rocketdebugger from "@/plugins/rocketdebugger";

export default {
  name: "RocketWSStatus",

  data: () => ({
    status: '',
    statusText: '',
    rawStatus: WebSocket.CLOSED
  }),
  watch: {
    rawStatus: function() {
      switch (rocketdebugger.getStatus()) {
        case WebSocket.CONNECTING:
          this.status = 'ws-green ws-pending';
          this.statusText = "Connecting...";
          break;
        case WebSocket.OPEN:
          this.status = 'ws-green';
          this.statusText = "Connected";
          break;
        case WebSocket.CLOSING:
          this.status = 'ws-red ws-pending';
          this.statusText = "Disconnecting...";
          break;
        case WebSocket.CLOSED:
          this.status = 'ws-red';
          this.statusText = "Disconnected.";
          break;
        default:
          this.status = 'ws-red ws-pending';
          this.statusText = "";
      }
    }
  },
  methods: {
    setState(state) {
      this.rawStatus = state;
    }
  },
  created() {
    rocketdebugger.addEventListener('onStateChanged', this.setState.bind(this));
  }
}
</script>

<style scoped lang="scss">
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
