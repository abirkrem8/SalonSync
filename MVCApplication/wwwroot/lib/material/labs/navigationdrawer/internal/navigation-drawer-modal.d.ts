/**
 * @license
 * Copyright 2021 Google LLC
 * SPDX-License-Identifier: Apache-2.0
 */
import { LitElement, PropertyValues } from 'lit';
/**
 * b/265346501 - add docs
 *
 * @fires navigation-drawer-changed {CustomEvent<{opened: boolean}>}
 * Dispatched whenever the drawer opens or closes --bubbles --composed
 */
export declare class NavigationDrawerModal extends LitElement {
    opened: boolean;
    pivot: 'start' | 'end';
    protected render(): import("lit-html").TemplateResult<1>;
    private getScrimClasses;
    private getRenderClasses;
    protected updated(changedProperties: PropertyValues<NavigationDrawerModal>): void;
    private handleKeyDown;
    private handleScrimClick;
}
