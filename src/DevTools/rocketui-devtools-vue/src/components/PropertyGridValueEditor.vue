<template>
  <v-container fluid>
  <v-row dense>
    <v-col>
    <template v-if="editing" :title="property.type">
      <template v-if="property.type === 'RocketUI.Thickness'">
        <v-row no-gutters dense>
          <v-col>
            <v-text-field flat label="Top" outlined dense v-model="property.value.Top"></v-text-field>
          </v-col>
          <v-col>
            <v-text-field flat label="Right" outlined dense v-model="property.value.Right"></v-text-field>
          </v-col>
          <v-col>
            <v-text-field flat label="Bottom" outlined dense v-model="property.value.Bottom"></v-text-field>
          </v-col>
          <v-col>
            <v-text-field flat label="Left" outlined dense v-model="property.value.Left"></v-text-field>
          </v-col>
        </v-row>
      </template>
      <template v-else-if="property.type === 'RocketUI.Size'">
        <v-row no-gutters dense>
          <v-col>
            <v-text-field flat label="Width" outlined dense v-model="property.value.Width"></v-text-field>
          </v-col>
          <v-col>
            <v-text-field flat label="Height" outlined dense v-model="property.value.Height"></v-text-field>
          </v-col>
        </v-row>
      </template>
      <template v-else-if="property.type === 'System.Boolean'">
        <v-switch v-model="property.value" :label="propertyName"/>
      </template>
      <template v-else-if="property.type === 'Microsoft.Xna.Framework.Color'">
        <v-color-picker
            disabled
            dot-size="12"
            hide-canvas
            mode="hexa"
            flat
            swatches-max-height="184"
            v-model="property.value"
        ></v-color-picker>
      </template>
      <template v-else>
        <v-text-field single-line flat dense outlined v-model="property.value" :label="propertyName"/>
      </template>
    </template>
    <template v-else-if="property.type === 'System.Boolean'">
      <v-switch flat v-model="property.value" :label="propertyName" readonly/>
    </template>
    <template v-else>
      {{property.value}}
    </template>
    </v-col>
    <v-col cols="1">
      <v-btn icon @click="toggleEdit" color="primary">
        <v-icon>mdi-pencil</v-icon>
      </v-btn>
    </v-col>
  </v-row>
  </v-container>
</template>

<script>
import {VBtn, VIcon} from "vuetify";

export default {
  name: 'PropertyGridValueEditor',
  data: () => ({
    editing: false
  }),
  components: {
    VBtn,
    VIcon
  },
  methods: {
    toggleEdit() {
      this.editing = !this.editing;
    }
  },
  props: {
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