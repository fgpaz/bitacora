'use client';

/**
 * ShellMenu — overflow menu para acciones del shell (logout + links de configuracion).
 * Protege acciones destructivas con fricción semántica (canon 12) y habilita
 * descubribilidad de configuración (Patron 16 #12 "Puente de siguiente acción").
 *
 * A11y: aria-haspopup=menu + aria-expanded; Esc cierra y retorna foco al trigger;
 * click-outside cierra.
 */

import { ReactNode, useEffect, useRef, useState } from 'react';
import styles from './ShellMenu.module.css';

interface ShellMenuProps {
  children: ReactNode;
  label?: string;
}

export function ShellMenu({ children, label = 'Mi cuenta' }: ShellMenuProps) {
  const [open, setOpen] = useState(false);
  const triggerRef = useRef<HTMLButtonElement | null>(null);
  const containerRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!open) return;

    function onKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') {
        e.stopPropagation();
        setOpen(false);
        requestAnimationFrame(() => triggerRef.current?.focus());
      }
    }

    function onClickOutside(e: MouseEvent) {
      if (!containerRef.current) return;
      if (!containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }

    document.addEventListener('keydown', onKeyDown);
    document.addEventListener('mousedown', onClickOutside);
    return () => {
      document.removeEventListener('keydown', onKeyDown);
      document.removeEventListener('mousedown', onClickOutside);
    };
  }, [open]);

  return (
    <div ref={containerRef} className={styles.menuContainer}>
      <button
        ref={triggerRef}
        type="button"
        className={styles.menuTrigger}
        aria-haspopup="menu"
        aria-expanded={open}
        aria-label={label}
        onClick={() => setOpen((v) => !v)}
      >
        <span aria-hidden="true" className={styles.menuTriggerIcon}>
          &#x22EF;
        </span>
      </button>
      <ul
        role="menu"
        hidden={!open}
        className={styles.menuList}
        aria-label={label}
      >
        {children}
      </ul>
    </div>
  );
}

interface ShellMenuItemProps {
  onClick: () => void;
  children: ReactNode;
  variant?: 'default' | 'destructive';
}

export function ShellMenuItem({ onClick, children, variant = 'default' }: ShellMenuItemProps) {
  const className =
    variant === 'destructive'
      ? `${styles.menuItem} ${styles.menuItemDestructive}`
      : styles.menuItem;

  return (
    <li role="none" className={styles.menuItemWrapper}>
      <button
        type="button"
        role="menuitem"
        onClick={onClick}
        className={className}
      >
        {children}
      </button>
    </li>
  );
}

export function ShellMenuSeparator() {
  return <li role="separator" aria-hidden="true" className={styles.menuSeparator} />;
}
