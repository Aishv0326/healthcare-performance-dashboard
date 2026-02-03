import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
    palette: {
        primary: { main: "#0074B7" },
        secondary: { main: "#60A3D9" },
        background: {
            default: "#BFD7ED",
            paper: "#ffffff",
        },
        text: {
            primary: "#003B73",
        },
    },
    shape: { borderRadius: 14 },
    typography: {
        fontFamily: "Inter, system-ui, Arial, sans-serif",
        h4: { fontWeight: 800 },
        h6: { fontWeight: 700 },
    },
    components: {
        MuiPaper: {
            styleOverrides: {
                root: {
                    borderRadius: 14,
                },
            },
        },
    },
});
