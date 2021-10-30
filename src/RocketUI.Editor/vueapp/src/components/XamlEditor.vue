<template>
  <MonacoEditor class="editor" v-model="xaml" language="xml" />
</template>

<script>
import MonacoEditor from 'vue-monaco';
import {mapActions, mapState} from "vuex";

export default {
  name: "XamlEditor",
  components: {
    MonacoEditor
  },
  computed: {
    ...mapState('elementTree', {
      element: state => state.selectedElement,
      xaml: state => state.selectedElementXaml
    }),
  },
  data: () => ({
    code: 'const noop = () => {};'
  }),
  watch: {
    element() {
      this.loadXaml();
    },
    xaml(newValue) {
      this.code = newValue;
    }
  },

  methods: {
    ...mapActions('elementTree', [
      'loadElementXaml'
    ]),
    loadXaml() {
      this.loadElementXaml(this.element.id);
    }
  }
}
</script>

<style scoped>
.editor {
  width: 100%;
  height: 100%;
  flex: 1;
}
</style>
