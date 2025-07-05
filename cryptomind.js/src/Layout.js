import NavigationBar from './Navigation_Bar/NavigationBar';
import { Outlet } from 'react-router-dom';
import './App.css';

export default function Layout() {
  
  return (
    <>

      <header>
        <NavigationBar />
      </header>
      <main>
        <Outlet />
      </main>
    </>
  );
}