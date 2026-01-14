import { NavItem } from '../models/nav-item.model';

export const MENU_ITEMS: NavItem[] = [
  { label: 'Dashboard', icon: 'layout-dashboard', route: '/dashboard' },
  { label: 'Products', icon: 'package', route: '/products' },
  { label: 'Categories', icon: 'tags', route: '/categories' },
  { label: 'Purchases', icon: 'shopping-cart', route: '/purchases' },
  { label: 'Suppliers', icon: 'truck', route: '/suppliers' },
];