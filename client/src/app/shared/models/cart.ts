import {nanoid} from 'nanoid'

export type CartType = {
    id: string;
    items: CartItem[];
    deliveryMethodId?: number;
    clientSecret?: string;
    coupon?: Coupon;
    paymentIntentId?: string;
}

export type CartItem = {
    productId: number;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
    brand: string;
    type: string;
}

export type Coupon = {
    name: string;
    amountOff?: number;
    percentOff?: number;
    promotionCode: string;
    couponId: string;
}

export class Cart implements CartType {
    id = nanoid();
    items: CartItem[] = [];
    deliveryMethodId?: number;
    clientSecret?: string;
    paymentIntentId?: string;
    coupon?: Coupon;
}