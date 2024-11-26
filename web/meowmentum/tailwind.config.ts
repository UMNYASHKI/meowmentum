import type { Config } from 'tailwindcss';

const { nextui } = require('@nextui-org/react');

const config: Config = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
    './node_modules/@nextui-org/theme/dist/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      screens: {
        tablet: '820px',
      },
      colors: {
        background: {
          light: '#ffffff',
          dark: '#282828',
        },
        button: {
          hover: '#3e3a3a',
        },
        primary: {
          DEFAULT: '#282828',
        },
      },
    },
  },
  darkMode: 'class',
  plugins: [nextui()],
};
export default config;
