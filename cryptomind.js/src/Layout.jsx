import Footer from './components/Footer.jsx';
import Navbar from './components/Navbar.jsx';
import { Outlet, useLocation } from 'react-router-dom';


export default function Layout() {
  const location = useLocation();
  const isAdmin = location.pathname.startsWith('/admin');
  return (
    <>
      {!isAdmin && <header>
        <Navbar />
      </header>
      }
      <main>
        <Outlet />
      </main>
      {!isAdmin && (
        <footer>
          <Footer />
        </footer>
      )}
    </>
  );
}