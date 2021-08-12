<template>
  <multipane class="panes" layout="vertical">
    <div class="pane">
      <element-toolbar />
      <ScreenTreeView/>
    </div>
    <multipane-resizer/>
    <div class="pane">
      <v-card>
        <ElementPropertyGrid/>
      </v-card>
    </div>
  </multipane>
</template>

<script>
import {Multipane, MultipaneResizer} from 'vue-multipane';
import ScreenTreeView from "@/components/ScreenTreeView";
import ElementPropertyGrid from "@/components/ElementPropertyGrid";
import ElementToolbar from "@/components/ElementToolbar";

export default {
  name: 'DevToolsMain',

  components: {
    ElementToolbar,
    ElementPropertyGrid,
    ScreenTreeView,
    Multipane,
    MultipaneResizer,
  },

  data: () => ({}),
}
</script>

<style lang="scss">
.panes {
  display: table;
  height: 100%;
  width: 100%;
  flex-flow: row nowrap;
  justify-content: stretch;
  align-items: stretch;
}

.panes > .pane {
  display: table-cell;
  text-align: left;
  overflow: hidden;
  min-width: 200px;
  background: #1A1A1A;
}

.panes > .pane:first-child {
  // flex: 1;
}

.panes > .pane ~ .pane {
}

.panes > .multipane-resizer {
  display: table-cell;
  margin: 0;
  left: 0;
  position: relative;
  background: map-get($material-theme, 'surface');

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
</style>
