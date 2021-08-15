<template>
  <splitpanes>
    <pane>
      <element-toolbar/>
      <ScreenTreeView/>
    </pane>
    <pane>
      <v-card>
        <ElementPropertyGrid/>
      </v-card>
    </pane>
  </splitpanes>
</template>

<script>
import ScreenTreeView from "@/components/ScreenTreeView";
import ElementPropertyGrid from "@/components/ElementPropertyGrid";
import ElementToolbar from "@/components/ElementToolbar";
import {Pane, Splitpanes} from "splitpanes/src/components/splitpanes";

export default {
  name: 'DevToolsMain',

  components: {
    ElementToolbar,
    ElementPropertyGrid,
    ScreenTreeView,
    Splitpanes,
    Pane
  },

  data: () => ({}),
}
</script>

<style lang="scss">
.panes {
  height: 100%;
}
.panes > .multipane-resizer {
  display: table-cell;
  margin: 0;
  left: 0;
  position: relative;
  background: map-get($material-dark, 'surface');

  &:before {
    display: block;
    content: "";
    width: 3px;
    height: 40px;
    position: absolute;
    top: 50%;
    left: 50%;
    margin-top: -20px;
    margin-left: -1.5px;
    border-left: 1px solid #ccc;
    border-right: 1px solid #ccc;
  }

  &:hover {
    &:before {
      border-color: #999;
    }
  }
}
.splitpanes__pane {
  max-height: calc(100vh - 24px);
}
</style>
