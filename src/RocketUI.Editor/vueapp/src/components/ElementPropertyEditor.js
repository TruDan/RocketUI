import {VAutocomplete, VCol, VColorPicker, VRow, VSwitch, VTextField} from "vuetify/lib/components";

export const EnumEditor = {
    name: 'EnumEditor',
    // functional: true,

    props: ["schema", "value"],

    computed: {
        values() {
            if (this.schema.isFlags) {
                return this.schema.enumValues.filter(x => (x.value & this.value) === this.value).map(x => x.value);
            } else {
                return this.value;
            }
        }
    },

    methods: {
        onChange(newValue) {
            if (this.schema.isFlags) {
                this.$emit('change', newValue.reduce((v1, v2) => (v1 | v2), 0));
            } else {
                this.$emit('change', newValue);
            }
        }
    },

    render() {
        return (<VAutocomplete
            items={this.schema.enumValues}
            itemText="key"
            itemValue="value"
            chips={this.schema.isFlags}
            multiple={this.schema.isFlags}
            value={this.values}
            {...{attrs: this.$attrs}}
            vOn:change={this.onChange}
        />);
    }
}
export const ObjectEditor = {
    name: 'ObjectEditor',
    functional: true,

    props: ["schema", "value"],

    render(h, {props, data, children}) {
        return (<VRow dense no-gutters>
            {Object.entries(props.value || [])
                .map(([k, v]) => (
                    <VCol key={k}>
                        <ElementPropertyEditor
                            schema={props.schema}
                            vModel={v}
                            label={k}
                            {...{attrs: data.attrs}}
                        />
                    </VCol>
                ))}
            {children}
        </VRow>)
    }
}

export const BooleanEditor = {
    name: 'BooleanEditor',

    props: ["value"],

    methods: {
        onChange(newValue) {
            this.$emit('change', !!newValue);
        }
    },

    render() {
        return (<VSwitch
            inputValue={this.value}
            {...{attrs: this.$attrs}}
            vOn:change={this.onChange}
        />);
    }
}

export const ElementPropertyEditor = {
    name: 'ElementPropertyEditor',

    functional: true,

    props: ["schema", "value"],

    render(h, {props, data, children}) {

        function getAppropriateEditorComponentType() {
            if (props.schema && props.schema.type === "System.Boolean") {
                return BooleanEditor;
            } else if (props.schema && props.schema.type === "Microsoft.Xna.Framework.Color") {
                return VColorPicker;
            } else if (props.schema && props.schema.enumValues) {
                return EnumEditor;
            } else if (props.schema && props.schema.type === "System.String") {
                return VTextField;
            } else if (typeof (props.value) === 'object') {
                return ObjectEditor;
            } else {
                return VTextField;
            }
        }

        const editable = (props.schema && props.schema.editable);
        return h(getAppropriateEditorComponentType(), {
            ...{
                attrs: {
                    ...data.attrs,
                    flat: false,
                    filled: editable,
                    hideCanvas: true,
                    mode: "hexa",
                    dotSize: 12,
                    dense: true,
                    hideDetails: "auto",
                    readonly: !editable,
                    disabled: !editable,
                    ...props,
                },
                class: {
                    'rounded-0': true
                },
                on: data.on
            }
        }, children);
    }
};

export default ElementPropertyEditor;
