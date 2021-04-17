<template>
  <v-list-item dense>
    <v-list-item-content>
      <v-list-item-title>{{ propertyName }}</v-list-item-title>
    </v-list-item-content>
    <v-list-item-content class="propeditor-value">
      <v-switch v-if="property.type === 'System.Boolean'"
                dense
                flat
                hide-details="auto"
                v-model="property.value" />
      <v-color-picker
          v-else-if="property.type === 'Microsoft.Xna.Framework.Color'"
          dot-size="12"
          hide-canvas
          mode="hexa"
          flat
          v-model="property.value"
      ></v-color-picker>
      <template v-else-if="typeof property.value ==='object'">
        <v-row dense no-gutters>
          <v-col v-for="(val, subPropertyName) in property.value" v-bind:key="subPropertyName">
            <v-text-field flat
                          hide-details="auto"
                          v-model="property.value[subPropertyName]"
                          v-bind:label="subPropertyName | capitalize"></v-text-field>
          </v-col>
        </v-row>
      </template>
      <template v-else>
        <v-text-field flat
                      single-line
                      hide-details="auto"
                      class="no-label"
                      v-model="property.value"/>
      </template>
    </v-list-item-content>
  </v-list-item>
</template>

<script>
import {mapActions} from 'vuex';
import rocketdebugger from "@/plugins/rocketdebugger";

export default {
  name: 'PropertyGridValueEditor',

  data: () => ({
    editing: false,
    isDirty: false,
    initialValue: undefined
  }),
  watch: {
    property: {
      deep: true,

      handler() {

       // if(this.initialValue === undefined) {
        //  this.initialValue = this.property;
        //  return;
        //}

       // if(this.initialValue !== this.property) {
       //   this.isDirty = true;
      //  }

      //  if(this.isDirty) {
       //   console.log("Wooot", this);
        console.log("Hello", this.elementId, this.propertyName, this.property.value);
        rocketdebugger.send('SetPropertyValue', this.elementId, this.propertyName, this.property.value);
        //this.setPropertyValue(this.elementId, this.propertyName, this.property.value);
       //   this.isDirty = false;
       // }
      }
    }
  },
  methods: {
    toggleEdit() {
      this.editing = !this.editing;
    },
    ...mapActions('elementTree', [
      'setPropertyValue'
    ])
  },
  props: {
    elementId: {
      type: String
    },
    propertyName: {
      type: String
    },
    property: {
      type: Object,

      default: () => ({value: null, type: ""})
    }
  }
}
</script>

<style lang="scss">

.v-input__slot {
  margin-bottom: 0;
}

.propeditor-value {
  flex-grow: 2 !important;
}

.v-color-picker {
  max-width: unset !important;

  .v-color-picker__controls {
    flex-direction: row;
    justify-content: flex-start;

    .v-color-picker__preview {
      max-width: 18rem;
    }

    .v-color-picker__edit {
      max-width: 16rem;
    }

    .v-color-picker__edit, .v-color-picker__preview {
      flex: 1 1;
      margin-left: .5rem;
      margin-right: .5rem;
      margin-top: 0;
    }
  }
}

.v-list-item__content {
  overflow: visible;
}

.theme--dark.v-text-field > .v-input__control > .v-input__slot:before {
  border-color: transparent !important;
}

.v-input.no-label {
  margin-top: 0 !important;
  padding-top: 0 !important;
}
.v-input--selection-controls {
  margin-top: 0 !important;
}

.v-input input[type="text"] {
  font-family: monospace;
  color: #eee !important;
}
</style>