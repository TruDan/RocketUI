<template>
  <v-sheet>
    <v-card>
      <v-card-title>{{ element.type }}</v-card-title>
      <v-card-text>
        <v-list>
          <v-list-item>
            <v-list-item-title>Id</v-list-item-title>
            <v-list-item-content>
              {{ element.properties.Id }}
            </v-list-item-content>
          </v-list-item>
        </v-list>
      </v-card-text>
    </v-card>
    <v-card>
      <v-card-title>Layout</v-card-title>
      <div class="layout-grid">
        <div class="layout-box" data-label="Margin">
          <span class="layout-text-top">{{ layoutMargin.top }}</span>
          <span class="layout-text-right">{{ layoutMargin.right }}</span>
          <span class="layout-text-bottom">{{ layoutMargin.bottom }}</span>
          <span class="layout-text-left">{{ layoutMargin.left }}</span>
          <div class="layout-box box-orange" data-label="Border">
            <span class="layout-text-top">{{ layoutBorder.top }}</span>
            <span class="layout-text-right">{{ layoutBorder.right }}</span>
            <span class="layout-text-bottom">{{ layoutBorder.bottom }}</span>
            <span class="layout-text-left">{{ layoutBorder.left }}</span>
            <div class="layout-box box-green" data-label="Padding">
              <span class="layout-text-top">{{ layoutPadding.top }}</span>
              <span class="layout-text-right">{{ layoutPadding.right }}</span>
              <span class="layout-text-bottom">{{ layoutPadding.bottom }}</span>
              <span class="layout-text-left">{{ layoutPadding.left }}</span>
              <div class="layout-box">
                {{ layoutContent.width }} x {{ layoutContent.height }}
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
        <v-tabs v-model="activeTab">
          <v-tab>All</v-tab>
          <v-tab>Editable</v-tab>
          <v-tab>Computed</v-tab>
        </v-tabs>
        <v-list>
          <template v-for="(prop, propName) in elementProperties">
            <property-grid-value-editor :key="propName"
                                        :label="propName"
                                        :name="propName"
                                        :schema="element.schema[propName]"
                                        :value="prop"
                                        @change="onPropertyValueChange"
            />
            <v-divider :key="propName + '1'"/>
          </template>
        </v-list>
      </v-card-text>
    </v-card>
  </v-sheet>
</template>

<script>
import {mapActions, mapState} from 'vuex';
import PropertyGridValueEditor from "@/components/PropertyGridValueEditor";

export default {
  name: "ElementPropertyGrid",
  computed: {
    ...mapState('elementTree', {
      element: state => state.selectedElement,
    }),

    layoutMargin() {
      if (!this.element || !this.element.properties.Margin) return {top: 0, left: 0, right: 0, bottom: 0};
      return this.element.properties.Margin;
    },
    layoutPadding() {
      if (!this.element || !this.element.properties.Padding) return {top: 0, left: 0, right: 0, bottom: 0};
      return this.element.properties.Padding;
    },
    layoutBorder() {
      if (!this.element || !this.element.properties.Border) return {top: 0, left: 0, right: 0, bottom: 0};
      return this.element.properties.Border;
    },
    layoutContent() {
      if (!this.element || !this.element.properties.ContentSize) return {width: 0, height: 0};
      return this.element.properties.ContentSize;
    },
    elementProperties() {
      if (this.activeTab === 2) {
        return Object.fromEntries(Object.entries(this.element.properties).filter(([p]) => !this.editableProps.includes(p)));
      } else if (this.activeTab === 1) {
        return Object.fromEntries(Object.entries(this.element.properties).filter(([p]) => this.editableProps.includes(p)));
      }

      return Object.fromEntries(Object.entries(this.element.properties));
    }
  },
  components: {
    PropertyGridValueEditor
  },

  data: () => ({
    activeTab: '',
    allProps: [],
    editableProps: []
  }),

  watch: {
    element() {
      this.organizeProperties();
    }
  },

  mounted() {
    if (this.element) {
      this.organizeProperties();
    }
  },

  methods: {
    ...mapActions('elementTree', [
      'setPropertyValue'
    ]),
    onPropertyValueChange({name, value}) {
      console.log('onPropertyValueChange', name, value);
      this.setPropertyValue({
        id: this.element.id,
        name,
        value
      });
    },
    organizeProperties() {
      var allProps = [];
      var editableProps = [];

      for (const [prop, schema] of Object.entries(this.element.schema)) {
        if (schema.editable) {
          editableProps.push(prop);
        }

        allProps.push(prop);
      }

      this.allProps = allProps;
      this.editableProps = editableProps;
    }
  }
}
</script>

<style scoped lang="scss">
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
      color: rgba(map-get($material-dark, 'text-color'), map-get($material-theme, 'secondary-text-percent'));
      font-weight: bold;
      position: absolute;
      top: $padding;
      left: $padding;
    }

    > span {
      padding: $padding;
      position: absolute;
      font-size: ($size - (4*$padding));
      color: rgba(map-get($material-dark, 'text-color'), map-get($material-theme, 'secondary-text-percent'));

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
