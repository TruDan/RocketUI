<template>
  <v-sheet>
    <v-card>
      <v-card-title>Layout</v-card-title>
      <div class="layout-grid">
        <div class="layout-box" data-label="Margin">
          <span class="layout-text-top">{{ layout.margin.top }}</span>
          <span class="layout-text-right">{{ layout.margin.right }}</span>
          <span class="layout-text-bottom">{{ layout.margin.bottom }}</span>
          <span class="layout-text-left">{{ layout.margin.left }}</span>
          <div class="layout-box box-orange" data-label="Border">
            <span class="layout-text-top">{{ layout.border.top }}</span>
            <span class="layout-text-right">{{ layout.border.right }}</span>
            <span class="layout-text-bottom">{{ layout.border.bottom }}</span>
            <span class="layout-text-left">{{ layout.border.left }}</span>
            <div class="layout-box box-green" data-label="Padding">
              <span class="layout-text-top">{{ layout.padding.top }}</span>
              <span class="layout-text-right">{{ layout.padding.right }}</span>
              <span class="layout-text-bottom">{{ layout.padding.bottom }}</span>
              <span class="layout-text-left">{{ layout.padding.left }}</span>
              <div class="layout-box">
                {{ layout.content.width }} x {{ layout.content.height }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </v-card>
    <v-divider/>
    <v-card>
      <v-card-title>Properties</v-card-title>
      <v-card-text>
        <v-list>
          <template v-for="(prop, propName) in element.properties">
            <PropertyGridValueEditor :key="propName" :element-id="element.id" :property-name="propName" :property="prop"/>
            <v-divider :key="propName + '1'"/>
          </template>
        </v-list>
      </v-card-text>
    </v-card>
  </v-sheet>
</template>

<script>
import {mapState} from 'vuex';
import PropertyGridValueEditor from "@/components/PropertyGridValueEditor";

export default {
  name: "ElementPropertyGrid",
  computed: {
    ...mapState({
      element: state => state.elementTree.selectedElement,
    })
  },
  watch: {
    element: (newVal) => {
      this.layout.margin = newVal.properties.margin;
      this.layout.padding = newVal.properties.padding;
      //this.layout.border = newVal.properties.border;
      this.layout.content = newVal.properties.size;
    }
  },
  components: {
    PropertyGridValueEditor
  },
  data: () => ({
    layout: {
      margin: {
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
      },
      border: {
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
      },
      padding: {
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
      },
      content: {
        width: 0,
        height: 0
      }
    }
  })
}
</script>

<style scoped lang="scss">
@import 'src/sass/main';

@debug $material-theme;

.layout-wrap {
  display: flex;
  align-items: center;
  justify-content: center;
  font-family: monospace;
  font-size: $font-size-root * 0.8;

  & > div {
    flex: 1;
  }
}

.layout-grid {
  $size: 1.5rem;
  $padding: 0.2rem;
  margin: 1rem auto;
  display: flex;
  align-items: center;
  justify-content: center;

  .layout-box {
    position: relative;
    display: inline-block;
    border: 1px solid transparent;
    //border: 1px solid rgba(map-get($light-blue, 'darken-1'), 0.6);
    //background: rgba(map-get($light-blue, 'base'), 0.2);
    //box-shadow: inset 0 0 0 1.5rem rgba(map-get($light-blue, 'base'), 0.2);
    min-width: 4rem;
    min-height: 4rem;

    margin: 1.5rem;
    isolation: isolate;

    &[data-label]::before {
      display: block;
      content: attr(data-label);
      font-size: ($size - (4*$padding));
      color: rgba(map-get($material-theme, 'text-color'), map-get($material-theme, 'secondary-text-percent'));
      font-weight: bold;
      position: absolute;
      top: $padding;
      left: $padding;
    }

    > span {
      padding: $padding;
      position: absolute;
      font-size: ($size - (4*$padding));
      color: rgba(map-get($material-theme, 'text-color'), map-get($material-theme, 'secondary-text-percent'));

      &.layout-text {
        width: $size;
        height: $size;
        line-height: 1;
        align-content: center;
        justify-content: center;
        display: inline-block;
        position: absolute;
        text-align: center;
        vertical-align: center;

        &-top {
          top: $padding;
          left: 50%;
          margin-left: -($size/2);
        }

        &-right {
          right: $padding;
          top: 50%;
          margin-top: -($size/2);
        }

        &-bottom {
          bottom: $padding;
          left: 50%;
          margin-left: -($size/2);
        }

        &-left {
          left: $padding;
          top: 50%;
          margin-top: -($size/2);
        }
      }

    }

    @each $name, $color in $colors {
      @if map-has-key($color, 'darken-1') and map-has-key($color, 'base') {
        &.box-#{$name} {
          border-color: rgba(map-get($color, 'darken-1'), 0.6);
          //          background-color: rgba(map-get($color, 'base'), 0.2);
          box-shadow: inset 0 0 0 1.5rem rgba(map-get($color, 'base'), 0.2);
        }
      }
    }

    &::before {
      display: block;
      content: attr(data-top);
      position: absolute;
      top: -1rem;
      left: 50%;
    }

    &::after {
      display: block;
      content: attr(data-bottom);
      position: absolute;
      bottom: -1rem;
      left: 50%;
    }

    & > span::before {
      display: block;
      content: attr(data-left);
      position: absolute;
      left: -1rem;
      top: 50%;
    }

    & > span::after {
      display: block;
      content: attr(data-right);
      position: absolute;
      right: -1rem;
      top: 50%;
    }
  }
}
</style>