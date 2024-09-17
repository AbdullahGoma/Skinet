import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from "../../../shared/pipes/address.pipe";
import { PaymentPipe } from "../../../shared/pipes/payment.pipe";

@Component({
  selector: 'app-order-detailed',
  standalone: true,
  imports: [
    MatCardModule,
    MatButton,
    DatePipe,
    CurrencyPipe,
    AddressPipe,
    PaymentPipe,
    RouterLink
],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss'
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService); // It injects the OrderService into the component without needing to explicitly declare it in the constructor.
  private activatedRoute = inject(ActivatedRoute);
  order?: Order;

  // Angular lifecycle hook
  ngOnInit(): void { // What should happen when a component is initialized.
    this.loadOrder();
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.orderService.getOrderDetailed(+id).subscribe({ // Since getOrderDetailed likely returns an Observable, subscribe() is used to listen for the response.
      next: order => this.order = order
    });

    // Observable: Lazy: starts the execution only when subscribe() is called.
    // Promise: Eager: starts the execution as soon as created.
  }
}
