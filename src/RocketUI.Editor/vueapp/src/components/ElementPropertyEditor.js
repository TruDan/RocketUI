import {VAutocomplete, VCol, VColorPicker, VRow, VSwitch, VTextField} from "vuetify/lib/components";

export const EnumEditor = {
    name: 'EnumEditor',
    functional: true,

    props: ["schema", "value"],

    render(h, {props, data, children}) {
        return h(VAutocomplete, {
            ...data,
            ...{
                attrs: {
                    items: props.schema.enumValues,
                    itemText: "key",
                    itemValue: "value",
                    chips: props.schema.isFlags,
                    multiple: props.schema.isFlags,
                    ...props,
                    ...(props.schema.isFlags ? {value: props.schema.enumValues.filter(x => (x.value & props.value) > 0).map(x => x.value)} : {})
                }
            }
        }, children);
    }
}
export const ObjectEditor = {
    name: 'ObjectEditor',
    functional: true,

    props: ["schema", "value"],

    render(h, {props, data, children}) {
        return (<VRow dense no-gutters>
            {Object.entries(props.value || []).map(([k, v]) => (<VCol key={k}>
                <ElementPropertyEditor
                    schema={props.schema}
                    value={v}
                    {...data.attrs} />
            </VCol>))}
            {children}
        </VRow>)
    }
}

export const ElementPropertyEditor = {
    name: 'ElementPropertyEditor',

    functional: true,

    props: ["schema", "value"],

    render(h, {props, data, children}) {

        function getAppropriateEditorComponentType() {
            if (props.schema && props.schema.type === "System.Boolean") {
                return VSwitch;
            } else if (props.schema && props.schema.type === "Microsoft.Xna.Framework.Color") {
                return VColorPicker;
            } else if (props.schema && props.schema.enumValues) {
                return EnumEditor;
            } else if(props.schema && props.schema.type === "System.String") {
                return VTextField;
            }
            else if (typeof (props.value) === 'object') {
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
                    singleLine: true,
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
