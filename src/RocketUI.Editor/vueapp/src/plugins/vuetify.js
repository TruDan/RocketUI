import Vue from 'vue';
import Vuetify from 'vuetify/lib/framework';
import colors from 'vuetify/es5/util/colors';

Vue.use(Vuetify);

export default new Vuetify({

    theme: {
        dark: true,
        options: {
            customProperties: true
        },
        themes: {
            light: {
                primary: colors.teal.base,
                secondary: colors.amber.base,
                accent: colors.lightBlue.base,
                error: colors.red.base,
                warning: colors.orange.base,
                info: colors.cyan.base,
                success: colors.lightGreen.base
            },
            dark: {
                primary: colors.teal.base,
                secondary: colors.amber.base,
                accent: colors.lightBlue.base,
                error: colors.red.base,
                warning: colors.orange.base,
                info: colors.cyan.base,
                success: colors.lightGreen.base
            }
        }
    },

});
