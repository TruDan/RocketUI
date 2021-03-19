import rocketdebugger from "@/plugins/rocketdebugger";

const state = () => ({
    elements: [],
    rootElements: [],
    selectedElement: null
});

const getters = {

};

const actions = {
    getRootElement({ commit }) {
        rocketdebugger.send('GetRoot', 128).then(root => {
            commit('setRoot', root);
        });
    },

    selectElement({commit}, elementId) {
        commit('setSelectedElement', elementId);
        rocketdebugger.send('SelectElement', elementId);

        // rocketdebugger.send('GetChildren', elementId).then(children => {
        //     commit('setChildrenOfElement', elementId, children);
        // });
        // rocketdebugger.send('GetProperties', elementId).then(props => {
        //     commit('setSelectedElementProperties', elementId, props);
        // });
    },
    setPropertyValue({commit}, elementId, propertyName, propertyValue) {
        rocketdebugger.send('SetPropertyValue', elementId, propertyName, propertyValue).then(success => {
            commit('setPropertyValue', elementId, propertyName, propertyValue);
            console.log("SetPropertyValue ", success, elementId, propertyName, propertyValue);
        });
    }
};

const mutations = {
    setRoot (state, root) {
        state.elements = [];
        //state.elements.clear();
        state.elements.push(root);
        state.rootElements = root;

        var discoverChildren;
        discoverChildren = (element) => {
            if ('children' in element) {
                if (element.children.length > 0) {
                    for (let child of element.children) {
                        state.elements.push(child);
                        discoverChildren(child);
                    }
                }
            }
        };

        for(var screen of root) {
            state.elements.push(screen);
            discoverChildren(screen);
        }
    },
    setSelectedElement(state, elementId) {
        const element = state.elements.find(x => x.id === elementId);
        state.selectedElement = element;
    },
    setChildrenOfElement(state, elementId, children) {
        const element = state.elements.find(x => x.id === elementId);
        state.elements.push(children);
        element.children = children;
    },
    setSelectedElementProperties(state, elementId, props){
        const element = state.elements.find(x => x.id === elementId);
        element.properties = props;
    },
    setPropertyValue(state, elementId, propertyName, propertyValue) {
        const element = state.elem.find(x => x.id === elementId);
        element.properties[propertyName] = propertyValue;
    }
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};