<template>
  <v-list-item dense>
    <v-list-item-content>
      <v-list-item-title>{{ propertyName }}</v-list-item-title>
    </v-list-item-content>
    <v-list-item-content>
      <v-switch v-if="property.type === 'System.Boolean'" dense flat hide-details="auto" :input-value="property.value"/>
      <v-color-picker
          v-else-if="property.type === 'Microsoft.Xna.Framework.Color'"
          disabled
          dot-size="12"
          hide-canvas
          mode="hexa"
          flat
          swatches-max-height="184"
          hide-details="auto"
          :value="property.value"
      ></v-color-picker>
      <template v-else-if="typeof property.value ==='object'">
        <v-row dense no-gutters>
          <v-col v-for="(val, subPropertyName) in property.value" v-bind:key="subPropertyName">
            <v-text-field flat hide-details="auto" :value="val"
                          v-bind:label="subPropertyName | capitalize"></v-text-field>
          </v-col>
        </v-row>
      </template>
      <template v-else>
        <v-text-field flat single-line hide-details="auto" :value="property.value"/>
      </template>
    </v-list-item-content>
  </v-list-item>
</template>

<script>
import {mapActions} from 'vuex';

export default {
  name: 'PropertyGridValueEditor',

  data: () => ({
    editing: false,
    isDirty: false
  }),
  methods: {
    toggleEdit() {
      this.editing = !this.editing;
    },
    ...mapActions('elementTree', [
      'setPropertyValue'
    ]),
    saveChanges() {
      if (this.isDirty) {
        this.setPropertyValue(this.elementId, this.propertyName, this.property.value)
      }
    }
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
</style>