module.exports = {
    content: ["**/*.razor", "**/*.cshtml", "**/*.cs", "**/*.html"],
    darkMode: 'class',
  theme: {
    extend: {
      screens: {
        'sm': '640px',
        'md': '800px',  // this is where you modify the breakpoint
        'lg': '1024px',
        'xl': '1400px',
        '2xl': '1536px',
      },
    },
  },
  plugins: [],
}

