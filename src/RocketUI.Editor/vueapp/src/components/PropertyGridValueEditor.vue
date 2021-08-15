<template>
  <v-list-item dense>
    <v-list-item-content>
      <v-list-item-title>{{ label }}</v-list-item-title>
    </v-list-item-content>
    <v-list-item-content class="propeditor-value">
      <element-property-editor
          :schema="schema"
          :value="value"
          @change="onChange"/>
    </v-list-item-content>
  </v-list-item>
</template>

<script>
import ElementPropertyEditor from "@/components/ElementPropertyEditor";

export default {
  name: 'PropertyGridValueEditor',
  components: {ElementPropertyEditor},
  data: () => ({
    editing: false,
    isDirty: false,
    initialValue: undefined
  }),
  methods: {
    toggleEdit() {
      this.editing = !this.editing;
    },
    onChange(value) {
      console.log('changed', value);
      this.$emit('change',{
        name: this.name,
        value: value
      });
    }
  },
  props: ["value", "label", "schema", "name"]
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
