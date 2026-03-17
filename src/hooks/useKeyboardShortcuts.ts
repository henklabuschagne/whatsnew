import { useEffect } from 'react';

interface ShortcutConfig {
  key: string;
  ctrl?: boolean;
  alt?: boolean;
  shift?: boolean;
  meta?: boolean;
  callback: (e: KeyboardEvent) => void;
  preventDefault?: boolean;
}

export function useKeyboardShortcut(config: ShortcutConfig) {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      const {
        key,
        ctrl = false,
        alt = false,
        shift = false,
        meta = false,
        callback,
        preventDefault = true
      } = config;

      const matches =
        e.key.toLowerCase() === key.toLowerCase() &&
        e.ctrlKey === ctrl &&
        e.altKey === alt &&
        e.shiftKey === shift &&
        e.metaKey === meta;

      if (matches) {
        if (preventDefault) {
          e.preventDefault();
        }
        callback(e);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [config]);
}

export function useKeyboardShortcuts(shortcuts: ShortcutConfig[]) {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      for (const shortcut of shortcuts) {
        const {
          key,
          ctrl = false,
          alt = false,
          shift = false,
          meta = false,
          callback,
          preventDefault = true
        } = shortcut;

        const matches =
          e.key.toLowerCase() === key.toLowerCase() &&
          e.ctrlKey === ctrl &&
          e.altKey === alt &&
          e.shiftKey === shift &&
          e.metaKey === meta;

        if (matches) {
          if (preventDefault) {
            e.preventDefault();
          }
          callback(e);
          break;
        }
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [shortcuts]);
}
