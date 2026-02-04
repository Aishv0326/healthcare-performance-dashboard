import ReactDOM from "react-dom/client";
import { ThemeProvider, CssBaseline } from "@mui/material";
import { theme } from "./theme/theme";
import Dashboard from "./pages/Dashboard";

// - Applies the global Material UI theme (colors, typography, etc.)
// - Applies CssBaseline (normalizes default browser styles)
// - Renders the main Dashboard page into the #root element
ReactDOM.createRoot(document.getElementById("root")).render(
  <ThemeProvider theme={theme}>
    {/* CssBaseline provides consistent styling across browsers */}
    <CssBaseline />
    <Dashboard />
  </ThemeProvider>,
);
