import { Component, OnInit } from "@angular/core"
import { BsModalRef, BsModalService, ModalOptions } from "ngx-bootstrap/modal"
// import { BsModalRef, BsModalService, ModalOptions } from "ngx-bootstrap/modal"
import { User } from "src/app/_models/user"
import { AdminService } from "src/app/_services/admin.service"
import { RolesModalComponent } from "src/app/modals/roles-modal/roles-modal.component"


@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {


  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>()
  users: User[] = []



  constructor(private bsModalService: BsModalService, private adminService: AdminService) { }




  ngOnInit(): void {
    this.getUserWithRoles()
  }

  getUserWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: response => this.users = response
    })
  }



  openRolesModal(user: User) {
    const initialState: ModalOptions = {
      initialState: {
        title: 'Hello modal',
        list: ['Hello world', 'Hello sun'],
        closeBtnName: 'Close',
      },
    }
    // this.bsModalRef = this.bsModalService.show(RolesModalComponent, initialState)
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const isConfirmUpdate = this.bsModalRef.content?.isConfirmUpdate
        const selectedRoles = this.bsModalRef.content?.selectedRoles.join(',')
        if (isConfirmUpdate && selectedRoles && selectedRoles !== "")
          this.adminService.updateUserRoles(user.username, selectedRoles).subscribe({
            next: response => user.roles = response
          })
      }
    })
    // this.bsModalRef.content!.closeBtnName = 'Close'
  }



}
