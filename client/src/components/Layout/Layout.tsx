import React, { ReactNode } from 'react';
import Navbar from './Navbar';
import './Layout.css';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="layout">
      <Navbar />
      <main className="main-content">
        {children}
      </main>
      <footer className="footer">
        <div className="container">
          <p>&copy; 2024 Blog Application. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
};

export default Layout;
